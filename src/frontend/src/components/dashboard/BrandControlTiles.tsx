import { useState, useEffect, useMemo } from 'react';
import { motion } from 'framer-motion';
import { ExternalLink, Shield, ShoppingCart, Truck, Layout } from 'lucide-react';
import api from '../../shared/services/api';

interface BrandFromApi {
    id: string;
    tenantId: string;
    name: string;
    description?: string;
    slogan?: string;
    logoUrl?: string;
    primaryColor?: string;
    secondaryColor?: string;
    configJson?: string;
    portalUrl?: string;
    sortOrder?: number;
    isActive?: boolean;
    IsActive?: boolean;
}

function resolveLogoUrl(logoUrl: string | null | undefined): string {
    if (!logoUrl || !logoUrl.trim()) return '';
    const u = logoUrl.trim();
    if (u.startsWith('http://') || u.startsWith('https://')) return u;
    const origin = typeof window !== 'undefined' ? window.location.origin : '';
    return u.startsWith('/') ? `${origin}${u}` : `${origin}/${u}`;
}

function parsePortFromConfig(configJson: string | undefined): number | null {
    if (!configJson) return null;
    try {
        const c = JSON.parse(configJson);
        const p = c?.port;
        if (typeof p === 'number') return p;
        if (typeof p === 'string') return parseInt(p, 10) || null;
        return null;
    } catch {
        return null;
    }
}

const BrandControlTiles = ({ metrics }: { metrics: any[] }) => {
    const [brands, setBrands] = useState<BrandFromApi[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const res = await api.get('/brands');
                const raw = res.data;
                const list = Array.isArray(raw) ? raw : (raw?.items && Array.isArray(raw.items) ? raw.items : []);
                if (!cancelled) {
                    setBrands(list.map((b: BrandFromApi & Record<string, unknown>) => ({
                        id: b.id ?? (b.Id as string),
                        tenantId: b.tenantId ?? (b.TenantId as string),
                        name: b.name ?? (b.Name as string) ?? '',
                        description: b.description ?? (b.Description as string) ?? b.slogan ?? (b.Slogan as string) ?? '',
                        slogan: b.slogan ?? (b.Slogan as string),
                        logoUrl: b.logoUrl ?? (b.LogoUrl as string),
                        primaryColor: b.primaryColor ?? (b.PrimaryColor as string) ?? '#10b981',
                        secondaryColor: b.secondaryColor ?? (b.SecondaryColor as string),
                        configJson: b.configJson ?? (b.ConfigJson as string),
                        portalUrl: b.portalUrl ?? (b.PortalUrl as string),
                        sortOrder: b.sortOrder ?? (b.SortOrder as number) ?? 999,
                        isActive: b.isActive ?? (b.IsActive as boolean) ?? true
                    })));
                }
            } catch (e) {
                console.error('Failed to fetch brands for control tiles', e);
                if (!cancelled) setBrands([]);
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, []);

    const mergedBrands = useMemo(() => {
        return brands.filter(b => b.isActive !== false).map(brand => {
            const metric = metrics?.find((m: any) => m.brandId === brand.tenantId || m.brandId === brand.id);
            const port = parsePortFromConfig(brand.configJson);
            return {
                ...brand,
                port: port ?? 5174,
                resolvedLogo: resolveLogoUrl(brand.logoUrl),
                metrics: metric ? {
                    revenue: `$${(metric.todayRevenue / 1000).toFixed(1)}k`,
                    orders: metric.todayOrders,
                    status: metric.status ?? 'Live'
                } : { revenue: '—', orders: '—', status: 'Live' }
            };
        }).sort((a, b) => {
            const orderDiff = (a.sortOrder ?? 999) - (b.sortOrder ?? 999);
            if (orderDiff !== 0) return orderDiff;
            return a.name.localeCompare(b.name);
        });
    }, [brands, metrics]);

    if (loading) {
        return (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="p-8 bg-white/5 rounded-3xl animate-pulse h-64" />
                <div className="p-8 bg-white/5 rounded-3xl animate-pulse h-64" />
            </div>
        );
    }

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {mergedBrands.map((brand) => {
                const baseUrl = (import.meta.env.PROD || !brand.port) && brand.portalUrl
                    ? (brand.portalUrl.startsWith('http') ? brand.portalUrl : `https://${brand.portalUrl}`)
                    : `${window.location.protocol}//${window.location.hostname}:${brand.port || '5173'}`;
                return (
                    <motion.div
                        key={brand.id}
                        whileHover={{ scale: 1.02, rotateY: 2 }}
                        transition={{ type: 'spring', stiffness: 300 }}
                        className="relative group p-8 bg-white/5 backdrop-blur-2xl border border-white/10 rounded-3xl overflow-hidden"
                    >
                        <div
                            className="absolute -top-20 -right-20 w-40 h-40 blur-[100px] opacity-20 transition-opacity group-hover:opacity-40"
                            style={{ backgroundColor: brand.primaryColor || '#10b981' }}
                        />
                        <div className="flex items-start justify-between mb-6">
                            <div className="flex items-center gap-4">
                                {brand.resolvedLogo ? (
                                    <img
                                        src={brand.resolvedLogo}
                                        alt=""
                                        className="w-12 h-12 object-contain rounded-lg bg-white/10"
                                    />
                                ) : (
                                    <div
                                        className="w-12 h-12 rounded-lg flex items-center justify-center text-xl font-bold text-white"
                                        style={{ backgroundColor: brand.primaryColor || '#10b981' }}
                                    >
                                        {(brand.name || '?').slice(0, 2).toUpperCase()}
                                    </div>
                                )}
                                <div>
                                    <h3 className="text-xl font-bold">{brand.name}</h3>
                                    <span className="text-[10px] text-emerald-400 font-bold uppercase tracking-widest">{brand.metrics.status}</span>
                                </div>
                            </div>
                            <div className="text-right">
                                <p className="text-xs text-slate-500 uppercase font-bold">Today</p>
                                <p className="text-lg font-mono font-bold text-white">{brand.metrics.revenue}</p>
                            </div>
                        </div>

                        <p className="text-sm text-slate-400 mb-8 leading-relaxed h-12 overflow-hidden">
                            {brand.description || brand.slogan || '—'}
                        </p>

                        <div className="grid grid-cols-2 gap-3">
                            <button
                                onClick={() => window.open(`${baseUrl}/admin`, '_blank')}
                                className="flex items-center justify-center gap-2 py-3 px-4 bg-white/10 hover:bg-white/20 rounded-xl text-xs font-bold transition-all border border-white/5"
                            >
                                <Shield size={14} className="text-slate-400" /> Admin
                            </button>
                            <button
                                onClick={() => window.open(`${baseUrl}/vendor`, '_blank')}
                                className="flex items-center justify-center gap-2 py-3 px-4 bg-white/10 hover:bg-white/20 rounded-xl text-xs font-bold transition-all border border-white/5"
                            >
                                <Layout size={14} className="text-slate-400" /> Vendor
                            </button>
                            <button
                                onClick={() => window.open(`${baseUrl}/`, '_blank')}
                                className="flex items-center justify-center gap-2 py-3 px-4 bg-white/10 hover:bg-white/20 rounded-xl text-xs font-bold transition-all border border-white/5"
                            >
                                <ShoppingCart size={14} className="text-slate-400" /> Customer
                            </button>
                            <button
                                onClick={() => window.open(`${baseUrl}/delivery`, '_blank')}
                                className="flex items-center justify-center gap-2 py-3 px-4 bg-white/10 hover:bg-white/20 rounded-xl text-xs font-bold transition-all border border-white/5"
                            >
                                <Truck size={14} className="text-slate-400" /> Delivery
                            </button>
                        </div>

                        <button
                            onClick={() => window.open(baseUrl, '_blank')}
                            className="w-full mt-6 flex items-center justify-center gap-2 py-3 bg-gradient-to-r from-slate-800 to-slate-900 hover:from-slate-700 hover:to-slate-800 rounded-xl text-xs font-bold transition-all group"
                        >
                            Launch Brand Control <ExternalLink size={12} className="group-hover:translate-x-1 group-hover:-translate-y-1 transition-transform" />
                        </button>
                    </motion.div>
                );
            })}

            <motion.div
                whileHover={{ scale: 1.02 }}
                className="flex flex-col items-center justify-center p-8 bg-dashed border-2 border-dashed border-white/10 rounded-3xl hover:border-white/20 hover:bg-white/5 transition-all text-slate-500 cursor-pointer"
            >
                <div className="w-12 h-12 rounded-full border border-slate-700 flex items-center justify-center mb-4 text-2xl font-light">+</div>
                <p className="text-sm font-bold">Onboard New Brand</p>
            </motion.div>
        </div>
    );
};

export default BrandControlTiles;

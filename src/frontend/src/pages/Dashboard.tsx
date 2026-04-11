import { useEffect, useMemo, useState } from 'react';
import { motion } from 'framer-motion';
import { ArrowUpRight } from 'lucide-react';
import CorporateLogo from '../images/RajeevsTech.png';
import api from '../shared/services/api';
import { useAuth } from '../shared/providers/AuthContext';
import { calculateBaseUrl, generateFinalUrl } from '../shared/utils/urlHelper';
import './Dashboard.css';

type RawBrand = Record<string, unknown>;
type Brand = {
  id: string; tenantId: string; name: string; description: string; slogan: string;
  logoUrl: string; primaryColor: string; secondaryColor: string; configJson: string;
  portalUrl: string; sortOrder: number; isVisible: boolean; port?: string;
};

const read = (item: RawBrand, ...keys: string[]) => String(keys.map((k) => item[k]).find((v) => v !== undefined && v !== null) ?? '');
const flag = (item: RawBrand, ...keys: string[]) => Boolean(keys.map((k) => item[k]).find((v) => v !== undefined) ?? true);
const num = (item: RawBrand, ...keys: string[]) => Number(keys.map((k) => item[k]).find((v) => v !== undefined) ?? 999);
const parseConfig = (json: string) => { try { return JSON.parse(json || '{}') as Record<string, string>; } catch { return {}; } };
const resolveLogoUrl = (logoUrl: string) => !logoUrl ? '' : logoUrl.startsWith('http') ? logoUrl : `${window.location.origin}${logoUrl.startsWith('/') ? '' : '/'}${logoUrl}`;

export default function Dashboard() {
  const { user } = useAuth();
  const [brands, setBrands] = useState<Brand[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/brands')
      .then(({ data }) => {
        const items = Array.isArray(data) ? data : Array.isArray(data?.items) ? data.items : [];
        setBrands(items.map((item: RawBrand) => ({
          id: read(item, 'id', 'Id'), tenantId: read(item, 'tenantId', 'TenantId'), name: read(item, 'name', 'Name'),
          description: read(item, 'description', 'Description', 'slogan', 'Slogan'), slogan: read(item, 'slogan', 'Slogan'),
          logoUrl: read(item, 'logoUrl', 'LogoUrl'), primaryColor: read(item, 'primaryColor', 'PrimaryColor') || '#0f172a',
          secondaryColor: read(item, 'secondaryColor', 'SecondaryColor') || '#f8fafc', configJson: read(item, 'configJson', 'ConfigJson') || '{}',
          portalUrl: read(item, 'portalUrl', 'PortalUrl'), sortOrder: num(item, 'sortOrder', 'SortOrder'), isVisible: flag(item, 'isActive', 'IsActive', 'isVisible', 'IsVisible'), port: read(item, 'port', 'Port') || undefined
        })));
      })
      .catch((err) => { console.error(err); setError('Failed to load brand portals.'); })
      .finally(() => setLoading(false));
  }, []);

  const visibleBrands = useMemo(() => Array.from(new Map(brands.filter((b) => b.isVisible && b.tenantId !== 'rajeev-pvt').map((b) => [b.tenantId, b])).values()).sort((a, b) => (a.sortOrder - b.sortOrder) || a.name.localeCompare(b.name)), [brands]);

  const openBrand = (brand: Brand) => {
    const config = parseConfig(brand.configJson);
    if (config.status === 'coming_soon' || brand.tenantId === 'morebrands') return;
    const baseUrl = calculateBaseUrl(brand);
    // let baseUrl = config.url || brand.portalUrl;
    const url = user ? generateFinalUrl(baseUrl, brand, user as never) : baseUrl;
    if (url) window.open(url, '_blank', 'noopener,noreferrer');
  };

  return (
    <div className="dashboard-container">
      <section className="dashboard-hero">
        <div className="hero-copy">
          <span className="hero-tag">Rajeev's Tech</span>
          <h1>Multi-brand dashboard for one shared SaaS platform.</h1>
          <p>`/` is now the launch page for GreenPantry, Omega Technologies, BangaruKottu, VanaVajram, VajraValli, and future brands. Each live tile opens its own domain in a new tab.</p>
          <div className="hero-stats">
            <div><strong>{visibleBrands.length}</strong><span>Brands</span></div>
            <div><strong>MultiTenantSaaS_DB_PRDDB</strong><span>Shared DB</span></div>
            <div><strong>EF Core</strong><span>Code First</span></div>
          </div>
        </div>
        <div className="hero-art">
          <img src={CorporateLogo} alt="Rajeev's Tech" />
        </div>
      </section>

      {error && <div className="dashboard-note error">{error}</div>}
      {loading && <div className="dashboard-note">Loading brand ecosystem...</div>}

      <section className="brand-grid">
        {visibleBrands.map((brand, index) => {
          const config = parseConfig(brand.configJson);
          const comingSoon = config.status === 'coming_soon' || brand.tenantId === 'morebrands';
          const target = brand.portalUrl || config.url || calculateBaseUrl(brand);
          return (
            <motion.button
              key={`${brand.tenantId}-${brand.id}-${index}`}
              type="button"
              className={`brand-tile ${comingSoon ? 'disabled' : ''}`}
              style={{ ['--brand-primary' as string]: brand.primaryColor, ['--brand-secondary' as string]: brand.secondaryColor }}
              initial={{ opacity: 0, y: 18 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: index * 0.06 }}
              onClick={() => openBrand(brand)} disabled={comingSoon}
            >
              <div className="tile-top">
                <div className="tile-logo">{brand.logoUrl ? <img src={resolveLogoUrl(brand.logoUrl)} alt={brand.name} /> : <span>{brand.name.slice(0, 2).toUpperCase()}</span>}</div>
                <span className={`tile-badge ${comingSoon ? 'muted' : ''}`}>{comingSoon ? 'Coming soon' : 'Live'}</span>
              </div>
              <h2>{brand.name}</h2>
              <p className="tile-slogan">{brand.slogan || brand.tenantId}</p>
              <p className="tile-copy">{brand.description}</p>
              <div className="tile-footer">
                <span></span>
                <span className="tile-cta">{comingSoon ? 'Await launch' : 'Open portal'} <ArrowUpRight size={16} /></span>
              </div>
            </motion.button>
          );
        })}
      </section>
    </div>
  );
}

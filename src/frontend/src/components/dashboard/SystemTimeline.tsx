import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { UserCheck, ShoppingCart, Key, ShieldAlert, Cpu, Bell } from 'lucide-react';
import api from '../../shared/services/api';
import { useAppConstants } from '../../shared/providers/AppConstantsContext';

const iconMap: Record<string, { icon: typeof UserCheck; color: string }> = {
    login: { icon: UserCheck, color: 'emerald' },
    order: { icon: ShoppingCart, color: 'blue' },
    auth: { icon: Key, color: 'purple' },
    alert: { icon: ShieldAlert, color: 'rose' },
    system: { icon: Cpu, color: 'amber' },
    info: { icon: Bell, color: 'slate' }
};

function timeAgo(date: string): string {
    const d = new Date(date);
    const now = new Date();
    const sec = Math.floor((now.getTime() - d.getTime()) / 1000);
    if (sec < 60) return 'Just now';
    if (sec < 3600) return `${Math.floor(sec / 60)}m ago`;
    if (sec < 86400) return `${Math.floor(sec / 3600)}h ago`;
    return `${Math.floor(sec / 86400)}d ago`;
}

interface NotificationItem {
    id: string;
    type: string;
    title: string;
    message: string;
    brandName?: string;
    timeAgo: string;
    createdAt: string;
}

const SystemTimeline = () => {
    const { get: getConstant } = useAppConstants();
    const [events, setEvents] = useState<NotificationItem[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const res = await api.get('/notifications');
                const list = Array.isArray(res.data) ? res.data : [];
                if (!cancelled) {
                    setEvents(list.slice(0, 8).map((n: any) => ({
                        id: n.id,
                        type: (n.type || 'info').toLowerCase(),
                        title: n.title || 'Notification',
                        message: n.message || '',
                        brandName: n.brandName,
                        timeAgo: n.timeAgo || timeAgo(n.createdAt),
                        createdAt: n.createdAt
                    })));
                }
            } catch {
                if (!cancelled) setEvents([]);
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, []);

    const displayEvents = events.length > 0
        ? events
        : [{ id: 'placeholder', type: 'info', title: getConstant('TIMELINE_EMPTY_TITLE', 'System Live Timeline'), message: getConstant('TIMELINE_EMPTY_MESSAGE', 'No recent activity. Events will appear here when you have notifications.'), timeAgo: '', createdAt: '' }];

    const typeLabel = (type: string) => {
        const labels: Record<string, string> = {
            login: 'User Login',
            order: 'New Order',
            auth: 'Security Event',
            alert: 'System Alert',
            system: 'Ecosystem Update',
            info: 'Info'
        };
        return labels[type] ?? 'Update';
    };

    return (
        <div className="p-6 bg-white/5 backdrop-blur-xl border border-white/10 rounded-3xl">
            <h3 className="text-lg font-bold mb-6">{getConstant('TIMELINE_HEADING', 'System Live Timeline')}</h3>

            {loading ? (
                <div className="space-y-4">
                    {[1, 2, 3].map(i => (
                        <div key={i} className="h-16 bg-white/5 rounded-xl animate-pulse" />
                    ))}
                </div>
            ) : (
                <div className="space-y-6 relative before:absolute before:inset-y-0 before:left-5 before:w-[1px] before:bg-white/10">
                    {displayEvents.map((event, i) => {
                        const { icon: Icon, color } = iconMap[event.type] ?? iconMap.info;
                        return (
                            <motion.div
                                key={event.id}
                                initial={{ opacity: 0, x: -10 }}
                                animate={{ opacity: 1, x: 0 }}
                                transition={{ delay: i * 0.05 }}
                                className="relative flex gap-4 pl-10"
                            >
                                <div className={`absolute left-0 w-10 h-10 rounded-full bg-[#0f172a] border border-${color}-500/50 flex items-center justify-center z-10 shadow-[0_0_10px_rgba(0,0,0,0.5)]`}>
                                    <Icon size={16} className={`text-${color}-400`} />
                                </div>
                                <div className="flex-1">
                                    <div className="flex items-center justify-between mb-1">
                                        <p className="text-xs font-bold text-slate-300">{event.title || typeLabel(event.type)}</p>
                                        {event.timeAgo && <span className="text-[10px] text-slate-500">{event.timeAgo}</span>}
                                    </div>
                                    <p className="text-sm text-slate-400">
                                        {event.brandName ? <span className="text-white">{event.brandName}</span> : null}
                                        {event.brandName && event.message ? ' — ' : null}
                                        {event.message || null}
                                    </p>
                                </div>
                            </motion.div>
                        );
                    })}
                </div>
            )}

            <button className="w-full mt-8 py-2 text-xs font-bold text-slate-500 hover:text-white transition-colors">
                {getConstant('TIMELINE_VIEW_ALL', 'View Full Audit Log')}
            </button>
        </div>
    );
};

export default SystemTimeline;

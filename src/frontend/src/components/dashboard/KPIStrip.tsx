import { motion } from 'framer-motion';
import { TrendingUp, ShoppingBag, Users, Cpu } from 'lucide-react';
import type { DashboardStats } from '../../types/dashboard';

const KPIStrip = ({ stats }: { stats: DashboardStats }) => {
    const data = [
        { label: 'Total Revenue', value: `$${(stats?.totalRevenue || 0).toLocaleString()}`, trend: '+12.5%', icon: TrendingUp, color: 'emerald', progress: '70%' },
        { label: 'Orders Today', value: (stats?.ordersToday || 0).toLocaleString(), trend: '+8.2%', icon: ShoppingBag, color: 'blue', progress: '65%' },
        { label: 'Active Vendors', value: (stats?.activeVendors || 0).toString(), trend: '+2.4%', icon: Users, color: 'purple', progress: '40%' },
        { label: 'System Load', value: `${stats?.systemLoad || 0}%`, trend: '-4.1%', icon: Cpu, color: 'amber', progress: `${stats?.systemLoad || 0}%` },
    ];

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {data.map((stat, index) => (
                <motion.div
                    key={stat.label}
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ delay: index * 0.1 }}
                    whileHover={{ y: -5 }}
                    className="group relative overflow-hidden p-6 bg-white/5 backdrop-blur-md border border-white/10 rounded-2xl transition-all hover:bg-white/10"
                >
                    <div className="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
                        <stat.icon size={64} />
                    </div>

                    <div className="flex items-center gap-4 mb-4">
                        <div className={`p-2 rounded-lg bg-${stat.color}-500/20 text-${stat.color}-400`}>
                            <stat.icon size={20} />
                        </div>
                        <span className="text-xs font-bold text-slate-400 uppercase tracking-widest">{stat.label}</span>
                    </div>

                    <div className="flex items-end justify-between">
                        <h3 className="text-2xl font-mono font-bold text-white">{stat.value}</h3>
                        <span className={`text-xs font-bold ${stat.trend.startsWith('+') ? 'text-emerald-400' : 'text-rose-400'}`}>
                            {stat.trend}
                        </span>
                    </div>

                    <div className="mt-4 h-1 w-full bg-white/5 rounded-full overflow-hidden">
                        <motion.div
                            initial={{ width: 0 }}
                            animate={{ width: stat.progress }}
                            transition={{ duration: 1, delay: 0.5 }}
                            className={`h-full bg-${stat.color}-500 shadow-[0_0_10px_#10b981]`}
                        />
                    </div>
                </motion.div>
            ))}
        </div>
    );
};


export default KPIStrip;

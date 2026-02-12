import { Activity } from 'lucide-react';
import type { DashboardStats } from '../../types/dashboard';

const CorporateHeader = ({ stats }: { stats: DashboardStats }) => {
    return (
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-6 bg-white/5 backdrop-blur-xl border border-white/10 rounded-2xl shadow-2xl">
            <div className="flex items-center gap-4">
                <div className="w-12 h-12 rounded-full bg-emerald-500/20 flex items-center justify-center border border-emerald-500/50">
                    <Activity className="text-emerald-400 w-6 h-6 animate-pulse" />
                </div>
                <div>
                    <h1 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-white to-slate-400">
                        Rajeev Corporate Command
                    </h1>
                    <div className="flex items-center gap-2 text-xs text-slate-400 uppercase tracking-widest mt-1">
                        <span className="flex items-center gap-1"><span className="w-2 h-2 rounded-full bg-emerald-500"></span> System Live</span>
                        <span className="opacity-30">|</span>
                        <span>Uptime: 99.98%</span>
                    </div>
                </div>
            </div>

            <div className="flex flex-wrap items-center gap-6">
                <div className="flex items-center gap-8 px-8 border-x border-white/5">
                    <div className="text-center">
                        <p className="text-[10px] text-slate-500 uppercase font-bold tracking-tighter">Active Tenants</p>
                        <p className="text-xl font-mono font-bold text-white">0{(stats?.brandMetrics?.length || 0)}</p>
                    </div>
                    <div className="text-center">
                        <p className="text-[10px] text-slate-500 uppercase font-bold tracking-tighter">Online Users</p>
                        <p className="text-xl font-mono font-bold text-emerald-400">1,284</p>
                    </div>
                    <div className="text-center">
                        <p className="text-[10px] text-slate-500 uppercase font-bold tracking-tighter">Revenue Today</p>
                        <p className="text-xl font-mono font-bold text-white">${((stats?.totalRevenue || 0) / 1000).toFixed(1)}k</p>
                    </div>
                </div>

                <button className="px-4 py-2 bg-gradient-to-r from-emerald-600 to-teal-500 hover:from-emerald-500 hover:to-teal-400 rounded-lg text-sm font-bold transition-all shadow-[0_0_20px_rgba(16,185,129,0.3)]">
                    System Analytics
                </button>
            </div>
        </div>
    );
};




export default CorporateHeader;

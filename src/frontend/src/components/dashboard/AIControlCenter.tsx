import { motion } from 'framer-motion';
import { Bot, FileText, LineChart, AlertTriangle, Settings } from 'lucide-react';
import { useAppConstants } from '../../shared/providers/AppConstantsContext';

const AIControlCenter = () => {
    const { get: getConstant } = useAppConstants();

    const actions = [
        { labelKey: 'AI_ACTION_REPORT', default: 'Generate Weekly Report', icon: FileText },
        { labelKey: 'AI_ACTION_TREND', default: 'Predict Sales Trend', icon: LineChart },
        { labelKey: 'AI_ACTION_ANOMALIES', default: 'Detect Anomalies', icon: AlertTriangle, color: 'rose' },
        { labelKey: 'AI_ACTION_PERF', default: 'Optimize Performance', icon: Settings },
    ];

    return (
        <div className="p-6 bg-gradient-to-br from-indigo-900/40 to-slate-900/40 backdrop-blur-2xl border border-indigo-500/20 rounded-3xl">
            <div className="flex items-center gap-3 mb-6">
                <div className="p-2 bg-indigo-500/20 rounded-lg">
                    <Bot className="text-indigo-400" />
                </div>
                <h3 className="text-lg font-bold">{getConstant('AI_CONTROL_HEADING', 'AI Control Center')}</h3>
            </div>

            <div className="space-y-3">
                {actions.map(({ labelKey, default: defaultLabel, icon: Icon, color }) => (
                    <motion.button
                        key={labelKey}
                        whileHover={{ x: 4 }}
                        className="w-full flex items-center gap-3 p-3 bg-white/5 hover:bg-white/10 rounded-xl text-sm transition-all border border-white/5 group"
                    >
                        <Icon size={16} className={`text-${color || 'slate'}-400 group-hover:text-indigo-400 transition-colors`} />
                        <span className="flex-1 text-left">{getConstant(labelKey, defaultLabel)}</span>
                        <span className="text-[10px] bg-indigo-500/20 text-indigo-400 px-2 py-0.5 rounded uppercase font-bold opacity-0 group-hover:opacity-100 transition-opacity">Run</span>
                    </motion.button>
                ))}
            </div>

            <div className="mt-6 p-4 bg-indigo-500/10 border border-indigo-500/20 rounded-xl">
                <p className="text-[10px] text-indigo-300 font-bold uppercase tracking-widest mb-1">
                    {getConstant('AI_SUGGESTION_LABEL', 'AI Suggestion')}
                </p>
                <p className="text-sm text-indigo-100 leading-relaxed italic">
                    "{getConstant('AI_SUGGESTION', 'One of your brands has seen increased activity. Consider reviewing capacity and support hours.')}"
                </p>
            </div>
        </div>
    );
};

export default AIControlCenter;

const { execSync } = require('child_process');

// We exclude node.exe from the auto-kill by name to avoid killing the npm/node orchestration process itself.
// However, we MUST kill processes on specific ports even if they are 'node' (like lingering vite servers).
const processesToKillByName = ['dotnet.exe', 'ApiOrchestrator.exe', 'VSTest.Console.exe', 'VBCSCompiler.exe'];
const portsToClean = [5114, 5173, 5174, 5175, 5176, 7001, 7002];

console.log('--- SYSTEM CLEANUP START ---');

// 1. Kill by name (mostly for sticky backend services)
processesToKillByName.forEach(proc => {
    try {
        console.log(`- Terminating process by name: ${proc}...`);
        execSync(`taskkill /F /IM ${proc} /T 2>nul || exit 0`, { stdio: 'ignore' });
    } catch (e) {
        // Ignore
    }
});

// 2. Kill by port (for UI servers like Vite and APIs)
portsToClean.forEach(port => {
    try {
        // Find PID listening on port
        // Use a safer command structure for Windows
        const output = execSync(`netstat -ano`, { encoding: 'utf8' });
        const lines = output.split('\n').filter(l => l.includes(`:${port}`) && l.includes('LISTENING'));

        lines.forEach(line => {
            const parts = line.trim().split(/\s+/);
            const pid = parts[parts.length - 1];

            if (pid && pid !== '0' && pid !== process.pid.toString()) {
                console.log(`- Port ${port} is occupied by PID ${pid}. Terminating...`);
                execSync(`taskkill /F /PID ${pid} /T 2>nul || exit 0`, { stdio: 'ignore' });
            }
        });
    } catch (e) {
        // Port is likely free
    }
});

console.log('--- SYSTEM CLEANUP COMPLETE ---');
process.exit(0);

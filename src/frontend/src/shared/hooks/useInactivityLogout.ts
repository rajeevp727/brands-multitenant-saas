import { useEffect, useRef, useCallback } from 'react';

const INACTIVITY_LIMIT = 60 * 1000; // 1 minute

export const useInactivityLogout = (onLogout: () => void) => {
    const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    const resetTimer = useCallback(() => {
        if (timerRef.current) {
            clearTimeout(timerRef.current);
        }
        timerRef.current = setTimeout(() => {
            console.log("Auto-logout triggered due to inactivity.");
            onLogout();
        }, INACTIVITY_LIMIT);
    }, [onLogout]);

    useEffect(() => {
        // Initial timer start
        resetTimer();

        // Events to listen for
        const events = ['mousemove', 'keydown', 'click', 'scroll', 'touchstart'];

        const handleActivity = () => {
            resetTimer();
        };

        // Attach listeners
        events.forEach(event => {
            window.addEventListener(event, handleActivity);
        });

        // Cleanup
        return () => {
            if (timerRef.current) {
                clearTimeout(timerRef.current);
            }
            events.forEach(event => {
                window.removeEventListener(event, handleActivity);
            });
        };
    }, [resetTimer]);
};

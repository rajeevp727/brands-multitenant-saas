import React, { useState, useEffect } from 'react';
import { isOnline, setupOfflineListener } from '../utils/offlineFallback';
import '../Components/styles.css';

/**
 * Offline Banner Component
 * Displays a banner when the app goes offline
 */
export default function OfflineBanner() {
  const [online, setOnline] = useState(isOnline());
  const [showBanner, setShowBanner] = useState(false);

  useEffect(() => {
    setupOfflineListener(
      () => {
        setOnline(true);
        setShowBanner(false);
      },
      () => {
        setOnline(false);
        setShowBanner(true);
      }
    );

    // Initial check
    setOnline(isOnline());
  }, []);

  if (!showBanner || online) {
    return null;
  }

  return (
    <div
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        background: 'rgba(56, 242, 230, 0.95)',
        color: '#000',
        padding: '12px 24px',
        textAlign: 'center',
        zIndex: 10000,
        fontWeight: 600,
        boxShadow: '0 2px 8px rgba(0, 0, 0, 0.2)',
      }}
    >
      📴 You're offline. Some features may be limited.
    </div>
  );
}

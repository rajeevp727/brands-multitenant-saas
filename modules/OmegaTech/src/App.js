import { BrowserRouter, Routes, Route } from "react-router-dom";
import Header from "./Components/Header";
import Footer from "./Components/Footer";
import OfflineBanner from "./Components/OfflineBanner";

import Home from "./Pages/Home";
import About from "./Pages/About";
import Contact from "./Pages/Contact";
import Careers from "./Pages/Careers";

import { useEffect, useState } from "react";


function App() {
  const [isInitializing, setIsInitializing] = useState(true);


  // Sync theme with dashboard cookie (Heartbeat)
  useEffect(() => {
    const getCookie = (name) => {
      const value = `; ${document.cookie}`;
      const parts = value.split(`; ${name}=`);
      if (parts.length === 2) return parts.pop().split(';').shift();
      return null;
    };

    const syncTheme = () => {
      const mode = getCookie('saas-theme-mode');
      if (mode === 'dark') {
        document.body.classList.add('dark');
      } else {
        document.body.classList.remove('dark');
      }
    };

    // Initial sync
    syncTheme();

    // Heartbeat poll
    const interval = setInterval(syncTheme, 1000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    const initialize = async () => {
      try {
        // Check URL params first (SSO)
        const params = new URLSearchParams(window.location.search);
        const ssoParam = params.get('sso');

        if (ssoParam) {
          try {
            // Robust Base64 decode (Unicode safe)
            const base64 = decodeURIComponent(ssoParam).replace(/ /g, '+');
            const rawData = decodeURIComponent(escape(atob(base64)));
            const ssoData = JSON.parse(rawData);
            const { token, primaryColor, secondaryColor } = ssoData;
            let user = ssoData.user;

            if (Array.isArray(user)) {
              user = user[0];
            }

            if (primaryColor) {
              const root = document.documentElement;
              root.style.setProperty('--primary-50', `${primaryColor}10`);
              root.style.setProperty('--primary-100', `${primaryColor}20`);
              root.style.setProperty('--primary-500', primaryColor);
              root.style.setProperty('--primary-600', primaryColor);
              root.style.setProperty('--color-primary-500', primaryColor); // Compatibility
            }
            if (secondaryColor) {
              const root = document.documentElement;
              root.style.setProperty('--secondary-500', secondaryColor);
              root.style.setProperty('--color-secondary-500', secondaryColor); // Compatibility
            }

            if (token) {
              localStorage.setItem('token', token);
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));
                console.log('✅ SSO session established for:', user.email);
              }
            }

            // Clean URL
            const url = new URL(window.location.href);
            url.searchParams.delete('sso');
            window.history.replaceState({}, '', url.toString());
          } catch (e) {
            console.error('Failed to parse SSO data:', e);
          }
        }
      } finally {
        setIsInitializing(false);
      }
    };

    initialize();
  }, []);

  if (isInitializing) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500"></div>
      </div>
    );
  }

  return (
    <BrowserRouter>
      <div className="app-layout">
        <OfflineBanner />
        <Header />

        <main className="app-content pb-16">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/about" element={<About />} />
            <Route path="/contact" element={<Contact />} />
            <Route path="/careers" element={<Careers />} />
          </Routes>
        </main>

        <Footer />
      </div>
    </BrowserRouter>
  );
}

export default App;

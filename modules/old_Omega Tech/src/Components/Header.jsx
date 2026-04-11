import { Link, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import "./styles.css";

export default function Header() {
  const location = useLocation();
  const [scrolled, setScrolled] = useState(false);
  const [theme, setTheme] = useState("dark");

  /* Scroll shadow */
  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 10);
    window.addEventListener("scroll", onScroll);
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  /* Default DARK theme */
  useEffect(() => {
    const savedTheme = localStorage.getItem("theme") || "dark";
    document.body.setAttribute("data-theme", savedTheme);
    setTheme(savedTheme);
  }, []);

  const toggleTheme = () => {
    const next = theme === "dark" ? "light" : "dark";
    document.body.setAttribute("data-theme", next);
    localStorage.setItem("theme", next);
    setTheme(next);
  };

  const currentPath = location.pathname;

  return (
    <header className={`header ${scrolled ? "header-scrolled" : ""}`}>
      <div className="header-content">
        {/* Logo */}
        <Link to="/" className="logo-box">
          <img
            src="/Template Images/Logo.jpg"
            alt="Omega Technologies logo"
            className="header-logo"
          />
          <div className="brand-text">
            <span className="brand-name">Omega Technologies</span>
          </div>
        </Link>

        {/* Navigation (Scrollable) */}
        <nav className="nav nav-scroll">
          <div className="nav-scroll-inner">
            {/* THEME TOGGLE — FIRST */}
            <button
              className="theme-toggle"
              onClick={toggleTheme}
              aria-label="Toggle theme"
            >
              {theme === "dark" ? "☀️" : "🌙"}
            </button>

            {/* HOME */}
            {currentPath === "/" ? (
              <span className="nav-cta">Home</span>
            ) : (
              <Link to="/">Home</Link>
            )}

            {/* ABOUT */}
            {currentPath === "/about" ? (
              <span className="nav-cta">About</span>
            ) : (
              <Link to="/about">About</Link>
            )}

            {/* CAREERS */}
            {currentPath === "/careers" ? (
              <span className="nav-cta">Careers</span>
            ) : (
              <Link to="/careers">Careers</Link>
            )}

            {/* CONTACT */}
            {currentPath === "/contact" ? (
              <span className="nav-cta">Contact</span>
            ) : (
              <Link to="/contact">Contact</Link>
            )}
          </div>
        </nav>
      </div>
    </header>
  );
}

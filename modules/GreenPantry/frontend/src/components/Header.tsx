import { Link, useNavigate } from 'react-router-dom'
import { Search, Menu, X } from 'lucide-react'
import SidebarContent from './SidebarContent'
import Cart from './Cart'
import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'

const Header = () => {
  const navigate = useNavigate()
  const [searchQuery, setSearchQuery] = useState('')
  const [isSidebarOpen, setIsSidebarOpen] = useState(false)

  // Close sidebar on ESC key
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape' && isSidebarOpen) {
        setIsSidebarOpen(false);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isSidebarOpen]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    if (searchQuery.trim()) {
      navigate(`/restaurants?search=${encodeURIComponent(searchQuery.trim())}`)
    }
  }

  return (
    <>
      <header className="header-container sticky top-0 z-50 bg-white/80 dark:bg-gray-900/80 backdrop-blur-md border-b border-gray-100 dark:border-gray-800">
        <div className="header-content-wrapper ml-[-1rem]">
          <div className="header-main-layout">
            {/* Left Side: Menu Button + Logo */}
            <div className="header-left-container">
              <motion.button
                onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                className="header-sidebar-toggle-button relative"
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                transition={{ duration: 0.1 }}
              >
                <AnimatePresence mode="wait">
                  {isSidebarOpen ? (
                    <motion.div
                      key="close"
                      initial={{ rotate: -90, opacity: 0 }}
                      animate={{ rotate: 0, opacity: 1 }}
                      exit={{ rotate: 90, opacity: 0 }}
                      transition={{ duration: 0.2, ease: "easeInOut" }}
                    >
                      <X className="w-6 h-6" />
                    </motion.div>
                  ) : (
                    <motion.div
                      key="menu"
                      initial={{ rotate: 90, opacity: 0 }}
                      animate={{ rotate: 0, opacity: 1 }}
                      exit={{ rotate: -90, opacity: 0 }}
                      transition={{ duration: 0.2, ease: "easeInOut" }}
                    >
                      <Menu className="w-6 h-6" />
                    </motion.div>
                  )}
                </AnimatePresence>
              </motion.button>

              <Link to="/" className="header-logo-container">
                <img
                  src="/GreenPantry logo.png"
                  alt="GreenPantry Logo"
                  className="header-logo-image"
                  onError={(e) => {
                    e.currentTarget.src = '/GreenPantry logo.png';
                  }}
                />
                <span className="header-logo-text">GreenPantry</span>
              </Link>
            </div>

            {/* Center: Search Bar */}
            <div className="header-center-container">
              <form onSubmit={handleSearch} className="header-search-form">
                <div className="header-search-input-wrapper">
                  <Search className="header-search-icon" />
                  <input
                    type="text"
                    placeholder="Search restaurants..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="header-search-input"
                  />
                </div>
              </form>
            </div>

            {/* Right Side: Stats + Cart */}
            <div className="header-right-container">
              {/* Cart */}
              <Cart />
            </div>
          </div>
        </div>
      </header>

      {/* Mobile Sidebar */}
      <AnimatePresence>
        {isSidebarOpen && (
          <motion.div
            className="fixed inset-0 bg-black bg-opacity-50 z-40"
            onClick={() => setIsSidebarOpen(false)}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.2 }}
          >
            <motion.div
              className="header-sidebar-container"
              onClick={(e: React.MouseEvent) => e.stopPropagation()}
              initial={{ x: -300 }}
              animate={{ x: 0 }}
              exit={{ x: -300 }}
              transition={{ duration: 0.3, ease: "easeInOut" }}
            >
              {/* Sidebar Header */}
              <div className="header-sidebar-header">
                <h2 className="header-sidebar-title">Menu</h2>
                <button
                  onClick={() => setIsSidebarOpen(false)}
                  className="header-close-button"
                >
                  <motion.div
                    whileHover={{ scale: 1.1 }}
                    whileTap={{ scale: 0.9 }}
                    transition={{ duration: 0.1 }}
                  >
                    <X className="w-6 h-6" />
                  </motion.div>
                </button>
              </div>

              {/* Sidebar Content */}
              <SidebarContent onClose={() => setIsSidebarOpen(false)} />
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </>
  )
}

export default Header
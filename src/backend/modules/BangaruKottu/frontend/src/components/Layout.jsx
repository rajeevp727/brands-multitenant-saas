import { useState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';

const getCookie = (name) => {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop()?.split(';').shift();
  return null;
};
import {
  LayoutDashboard,
  Package,
  ShoppingCart,
  User,
  LogOut,
  Menu,
  X
} from 'lucide-react';

const Layout = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const user = authService.getCurrentUser();

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  useEffect(() => {
    const syncTheme = () => {
      const theme = getCookie('saas-theme-mode') || 'light';
      document.documentElement.classList.toggle('dark', theme === 'dark');
    };
    syncTheme();
    // Re-sync on storage change (for other tabs)
    window.addEventListener('storage', syncTheme);
    return () => window.removeEventListener('storage', syncTheme);
  }, []);

  // Close sidebar on ESC key
  useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.key === 'Escape' && sidebarOpen) {
        setSidebarOpen(false);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [sidebarOpen]);

  const menuItems = [
    { path: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
    { path: '/orders', icon: ShoppingCart, label: 'Orders' },
    { path: '/products', icon: Package, label: 'Products' },
    { path: '/profile', icon: User, label: 'Profile' },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="flex h-screen overflow-hidden">
        <aside className={`fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-lg transform transition-transform duration-300 ease-in-out lg:translate-x-0 ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'} lg:static lg:inset-0`}>
          <div className="flex flex-col h-full">
            <div className="flex items-center justify-between h-16 px-6 border-b border-gray-200">
              <h1 className="text-xl font-bold text-primary-600">BangaruKottu</h1>
              <button
                onClick={() => setSidebarOpen(false)}
                className="lg:hidden text-gray-500 hover:text-gray-700"
              >
                <X size={24} />
              </button>
            </div>
            <nav className="flex-1 px-4 py-6 space-y-2">
              {menuItems.map((item) => {
                const Icon = item.icon;
                const isActive = location.pathname === item.path;
                return (
                  <Link
                    key={item.path}
                    to={item.path}
                    onClick={() => setSidebarOpen(false)}
                    className={`flex items-center px-4 py-3 text-sm font-medium rounded-lg transition-colors ${isActive
                      ? 'bg-primary-50 text-primary-700'
                      : 'text-gray-700 hover:bg-gray-100'
                      }`}
                  >
                    <Icon size={20} className="mr-3" />
                    {item.label}
                  </Link>
                );
              })}
            </nav>
            <div className="p-4 border-t border-gray-200">
              <button
                onClick={handleLogout}
                className="flex items-center w-full px-4 py-3 text-sm font-medium text-red-600 rounded-lg hover:bg-red-50 transition-colors"
              >
                <LogOut size={20} className="mr-3" />
                Logout
              </button>
            </div>
          </div>
        </aside>

        <div className="flex flex-col flex-1 overflow-hidden lg:ml-0">
          <header className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-30">
            <div className="flex items-center justify-between h-16 px-6">
              <button
                onClick={() => setSidebarOpen(true)}
                className="lg:hidden text-gray-500 hover:text-gray-700"
              >
                <Menu size={24} />
              </button>
              <div className="flex items-center space-x-4 ml-auto">
                <div className="text-right">
                  <p className="text-sm font-medium text-gray-900">{user?.vendorName}</p>
                  <p className="text-xs text-gray-500">{user?.email}</p>
                </div>
              </div>
            </div>
          </header>

          <main className="flex-1 overflow-y-auto p-6 pb-20">
            {children}
          </main>

          <footer className="bg-white border-t border-gray-200 py-3 px-6 fixed bottom-0 left-0 right-0 lg:left-64 z-30 flex justify-between items-center text-xs text-gray-500">
            <div>© {new Date().getFullYear()} BangaruKottu Jewelers. All rights reserved.</div>
            <div className="flex gap-4">
              <a href="/privacy" className="hover:text-primary-600">Privacy</a>
              <a href="/terms" className="hover:text-primary-600">Terms</a>
            </div>
          </footer>
        </div>
      </div >

      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}
    </div >
  );
};

export default Layout;


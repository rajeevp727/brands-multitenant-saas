import { Outlet } from 'react-router-dom'
import Header from './Header'
import Footer from './Footer'

const Layout = () => {
  return (
    <div className="min-h-screen flex flex-col">
      <header className="header-container sticky top-0 z-50 bg-white/80 dark:bg-gray-900/80 backdrop-blur-md border-b border-gray-100 dark:border-gray-800">
        <Header />
      </header>
      <main className="flex-1 pb-16 px-4 sm:px-6 lg:px-8">
        <Outlet />
      </main>
      <footer className="footer-container sticky bottom-0 z-50 bg-white/80 dark:bg-gray-900/80 backdrop-blur-md border-t border-gray-100 dark:border-gray-800">
        <Footer />
      </footer>
    </div>
  )
}

export default Layout

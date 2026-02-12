const Footer = () => {
  return (
    <footer className="footer-container py-4 border-t border-gray-100 dark:border-gray-800 fixed bottom-0 left-0 right-0 bg-white/90 dark:bg-gray-900/90 backdrop-blur-md z-40">
      <div className="footer-content-wrapper max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="footer-main-layout flex flex-col md:flex-row justify-between items-center gap-2">
          <p className="text-gray-500 dark:text-gray-400 text-[10px] font-medium">
            © {new Date().getFullYear()} GreenPantry. All rights reserved.
          </p>
          <div className="footer-links flex gap-4">
            <a href="/privacy" className="text-gray-400 hover:text-green-600 text-[9px] transition-colors">Privacy Policy</a>
            <a href="/terms" className="text-gray-400 hover:text-green-600 text-[9px] transition-colors">Terms of Service</a>
            <a href="/contact" className="text-gray-400 hover:text-green-600 text-[9px] transition-colors">Contact Us</a>
          </div>
        </div>
      </div>
    </footer>
  )
}

export default Footer
const Footer = () => {
  return (
    <footer className="w-full bg-white dark:bg-gray-900 border-t border-gray-200 dark:border-gray-800 py-8 mt-auto">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex flex-col md:flex-row justify-between items-center gap-4">
          <div className="flex items-center space-x-2">
            <span className="text-xl font-bold text-green-600 dark:text-green-500">GreenPantry</span>
          </div>
          <p className="text-gray-500 dark:text-gray-400 text-sm">
            © {new Date().getFullYear()} GreenPantry. All rights reserved.
          </p>
          <div className="flex gap-6">
            <a href="/privacy" className="text-sm font-medium text-gray-500 hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400 transition-colors">Privacy Policy</a>
            <a href="/terms" className="text-sm font-medium text-gray-500 hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400 transition-colors">Terms of Service</a>
            <a href="/contact" className="text-sm font-medium text-gray-500 hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400 transition-colors">Contact Us</a>
          </div>
        </div>
      </div>
    </footer>
  )
}

export default Footer
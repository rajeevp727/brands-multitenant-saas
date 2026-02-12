/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          50: 'var(--color-primary-50, #f0fdf4)',
          100: 'var(--color-primary-100, #dcfce7)',
          200: 'var(--color-primary-200, #bbf7d0)',
          300: 'var(--color-primary-300, #86efac)',
          400: 'var(--color-primary-400, #4ade80)',
          500: 'var(--color-primary-500, #22c55e)',
          600: 'var(--color-primary-600, #16a34a)',
          700: 'var(--color-primary-700, #15803d)',
          800: 'var(--color-primary-800, #166534)',
          900: 'var(--color-primary-900, #14532d)',
        },
        secondary: {
          50: 'var(--color-secondary-50, #fefce8)',
          100: 'var(--color-secondary-100, #fef9c3)',
          200: 'var(--color-secondary-200, #fef08a)',
          300: 'var(--color-secondary-300, #fde047)',
          400: 'var(--color-secondary-400, #facc15)',
          500: 'var(--color-secondary-500, #eab308)',
          600: 'var(--color-secondary-600, #ca8a04)',
          700: 'var(--color-secondary-700, #a16207)',
          800: 'var(--color-secondary-800, #854d0e)',
          900: 'var(--color-secondary-900, #713f12)',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      animation: {
        'fade-in': 'fadeIn 0.5s ease-in-out',
        'slide-up': 'slideUp 0.3s ease-out',
        'bounce-gentle': 'bounceGentle 2s infinite',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        slideUp: {
          '0%': { transform: 'translateY(10px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        bounceGentle: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-5px)' },
        },
      },
    },
  },
  plugins: [],
}

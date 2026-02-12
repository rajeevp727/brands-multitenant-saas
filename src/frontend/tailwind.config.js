/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                corporate: {
                    navy: "#0f172a",
                    slate: "#1e293b",
                    emerald: "#10b981",
                    white: "#f8fafc",
                },
            },
            backdropBlur: {
                xs: "2px",
            },
        },
    },
    plugins: [],
}

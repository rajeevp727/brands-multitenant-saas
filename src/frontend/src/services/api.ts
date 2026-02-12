import axios, { AxiosError } from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "/api",
    withCredentials: true, // Required for cookies
    headers: {
        "Content-Type": "application/json"
    }
});

// ==========================
// REQUEST INTERCEPTOR
// ==========================
api.interceptors.request.use(
    config => {
        // ---------- TENANT RESOLUTION ----------
        const hostname = window.location.hostname;

        // Dynamic Resolution (simplified for brevity, should use a registry ideally)
        const tenantMap: Record<string, string> = {
            localhost: "rajeev-pvt",
            "greenpantry.local": "greenpantry",
            "omega.local": "omega",
            "bangaru.local": "bangaru",
            "rajeevs-pvt-ltd.vercel.app": "rajeev-pvt",
            "green-pantry-saas.vercel.app": "greenpantry",
            "omega-technologies.vercel.app": "omega",
            "bangaru-kottu.vercel.app": "bangaru",
            "rajeevstech.in": "rajeev-pvt"
        };

        const tenantId = tenantMap[hostname] || "rajeev-pvt";
        config.headers["X-Tenant-Id"] = tenantId;

        // ---------- JWT RESOLUTION ----------
        // No longer reading from localStorage for security (HttpOnly Cookies used instead).
        // Browser handles sending the cookie automatically because withCredentials: true.

        return config;
    },
    error => Promise.reject(error)
);

// ==========================
// RESPONSE INTERCEPTOR
// ==========================
api.interceptors.response.use(
    response => response,
    (error: AxiosError) => {
        if (error.response?.status === 401) {
            const currentPath = window.location.pathname;

            // Prevent infinite redirect loop
            if (currentPath !== "/login") {
                console.warn("Unauthorized - redirecting to login");

                localStorage.removeItem("user");
                localStorage.removeItem("access_token");

                window.location.href = "/login";
            }
        }

        return Promise.reject(error);
    }
);

export default api;

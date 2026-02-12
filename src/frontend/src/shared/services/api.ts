import axios, { AxiosError } from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "/api",
    withCredentials: true,
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

        const tenantMap: Record<string, string> = {
            localhost: "rajeev-pvt",
            "greenpantry.local": "greenpantry",
            "omega.local": "omega",
            "bangaru.local": "bangaru",
            // Vercel Domains
            "rajeevs-pvt-ltd.vercel.app": "rajeev-pvt",
            "green-pantry-saas.vercel.app": "greenpantry",
            "omega-technologies.vercel.app": "omega",
            "bangaru-kottu.vercel.app": "bangaru",
            // Custom Domains
            "rajeevstech.in": "rajeev-pvt",
            "www.rajeevstech.in": "rajeev-pvt",
            "greenpantry.in": "greenpantry",
            "www.greenpantry.in": "greenpantry",
            "omega-technologies.in": "omega",
            "www.omega-technologies.in": "omega"
        };

        const tenantId = tenantMap[hostname] || "rajeev-pvt";
        config.headers["X-Tenant-Id"] = tenantId;

        // ---------- JWT RESOLUTION ----------
        // Primary source: user object
        const userStr = localStorage.getItem("user");
        if (userStr) {
            try {
                const user = JSON.parse(userStr);
                if (user?.token) {
                    config.headers.Authorization = `Bearer ${user.token}`;
                }
            } catch {
                console.warn("Invalid user object in localStorage");
            }
        }

        // Fallback source: access_token for legacy compatibility
        const directToken = localStorage.getItem("access_token");
        if (directToken && !config.headers.Authorization) {
            config.headers.Authorization = `Bearer ${directToken}`;
        }

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

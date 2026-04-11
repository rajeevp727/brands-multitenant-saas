import axios, { AxiosError } from "axios";

// ==========================
// TENANT RESOLUTION
// ==========================
const getTenantFromDomain = (): string | null => {
    const host = window.location.hostname;

    // ✅ Local development
    if (host.includes("localhost")) return "rajeev-pvt";

    // ✅ Extract subdomain safely
    const parts = host.split(".");
    if (parts.length > 2) {
        return parts[0];
    }

    return null; // fail-safe
};

// ==========================
// AXIOS INSTANCE
// ==========================
const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "/api",
    withCredentials: true, // 🔥 REQUIRED for cookies
    headers: {
        "Content-Type": "application/json"
    }
});

// ==========================
// REQUEST INTERCEPTOR
// ==========================
api.interceptors.request.use(
    (config) => {
        const hostname = window.location.hostname;

        // ✅ Centralized tenant mapping
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

        // ✅ Resolve tenant
        let tenantId: string | null = tenantMap[hostname] ?? null;
        if (!tenantId) {
            tenantId = getTenantFromDomain();
        }

        if (!tenantId) {
            console.error("❌ Tenant resolution failed for:", hostname);
            throw new Error("Tenant resolution failed");
        }

        // ✅ Attach tenant header
        config.headers["X-Tenant-Id"] = tenantId;

        // ✅ Optional JWT fallback (debug/dev)
        const token = localStorage.getItem("access_token");
        if (token) {
            config.headers["Authorization"] = `Bearer ${token}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

// ==========================
// RESPONSE INTERCEPTOR
// ==========================
api.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        if (error.response?.status === 401) {
            const currentPath = window.location.pathname;
            const requestUrl = error.config?.url ?? "";

            const isSilentCheck = requestUrl.includes("/auth/me");

            console.warn("⚠️ 401 Unauthorized:", {
                url: requestUrl,
                path: currentPath
            });

            if (currentPath !== "/login" && !isSilentCheck) {
                localStorage.removeItem("user");
                localStorage.removeItem("access_token");

                window.location.href = "/login";
            }
        }

        return Promise.reject(error);
    }
);

export default api;
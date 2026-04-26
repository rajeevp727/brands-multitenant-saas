import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./index.css"; // optional, keep if exists
import * as serviceWorkerRegistration from "./serviceWorkerRegistration";
import reportWebVitals from "./reportWebVitals";

const root = ReactDOM.createRoot(document.getElementById("root"));

root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// Register service worker for PWA functionality
// Pass a callback to handle updates
serviceWorkerRegistration.register({
  onSuccess: () => {
    console.log("✅ PWA: Service worker registered successfully");
  },
  onUpdate: (registration) => {
    console.log("🔄 PWA: New service worker available. Reload to update.");
    // Optional: Show update notification to user
    if (window.confirm("New version available! Reload to update?")) {
      registration.waiting?.postMessage({ type: "SKIP_WAITING" });
      window.location.reload();
    }
  },
});

// Report web vitals (optional)
reportWebVitals();


import "./styles.css";

export default function Services() {
  return (
    <section className="services-section">
      <div className="section-container">
        <h2>Our Services</h2>
        <p className="section-subtitle">
          End-to-end software engineering services tailored for modern businesses.
        </p>

        <div className="services-grid">

          <details className="service-card">
            <summary>
              <h3>Enterprise Web Applications</h3>
              <p>.NET Core, React, SQL Server based scalable systems.</p>
            </summary>
          </details>

          <details className="service-card">
            <summary>
              <h3>Cloud & DevOps</h3>
              <p>Azure cloud, CI/CD pipelines, infrastructure automation.</p>
            </summary>
          </details>

          <details className="service-card">
            <summary>
              <h3>API & Microservices</h3>
              <p>Secure, performant backend architectures.</p>
            </summary>
          </details>

          <details className="service-card">
            <summary>
              <h3>UI / UX Engineering</h3>
              <p>Clean, responsive, business-driven interfaces.</p>
            </summary>
          </details>

        </div>
      </div>
    </section>
  );
}

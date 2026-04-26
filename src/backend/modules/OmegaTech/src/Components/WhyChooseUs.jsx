import "./styles.css";

export default function WhyChooseUs() {
  return (
    <section className="why-section">
      <div className="section-container">
        <h2>Why Choose Omega Technologies</h2>
        <p className="section-subtitle">
          Quality, reliability, and long-term partnerships.
        </p>

        <div className="why-grid">

          <details className="why-card">
            <summary>
              <h4>Enterprise-First</h4>
              <p>Architected for scalability and maintainability.</p>
            </summary>
          </details>

          <details className="why-card">
            <summary>
              <h4>Proven Stack</h4>
              <p>.NET, React, SQL Server, Azure.</p>
            </summary>
          </details>

          <details className="why-card">
            <summary>
              <h4>Ownership</h4>
              <p>End-to-end responsibility for delivery.</p>
            </summary>
          </details>

          <details className="why-card">
            <summary>
              <h4>Security & Performance</h4>
              <p>Industry-grade security and optimized systems.</p>
            </summary>
          </details>

        </div>
      </div>
    </section>
  );
}

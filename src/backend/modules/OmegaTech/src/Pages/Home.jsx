import "../Components/styles.css";

export default function Home() {
  return (
    <>
      {/* HERO SECTION */}
      <section className="home-hero hero-bg">
        <div className="home-hero-inner">
          <div className="home-hero-copy">
            <p className="home-hero-kicker">Omega Technologies Pvt. Ltd.</p>

            <h1>Engineering Scalable Digital Platforms for Startups and Enterprises</h1>

            <p className="home-hero-subtitle">
              We build secure, cloud-native, high‑performance software using .NET, React, Cloud,
              and Automation – engineered for teams that need to scale with confidence.
            </p>

            <div className="home-hero-actions">
              <a href="/contact" className="btn-primary">
                Get a Quote
              </a>
              <a href="/about" className="btn-secondary">
                Learn More
              </a>
            </div>

            <div className="home-hero-meta">
              <span>30+ Systems Delivered</span>
              <span>Cloud‑Native</span>
              <span>Secure‑by‑Design</span>
              <span>Scale‑Ready</span>
            </div>
          </div>

          <div className="home-hero-highlight">
            <div className="home-hero-highlight-metric">
              <span className="metric-label">Enterprise Platforms</span>
              <span className="metric-value">10k+ users</span>
              <span className="metric-note">Built for regulated industries</span>
            </div>
            <div className="home-hero-highlight-metric">
              <span className="metric-label">Automation</span>
              <span className="metric-value">60% less manual ops</span>
              <span className="metric-note">Workflows, bots &amp; integrations</span>
            </div>
          </div>
        </div>
      </section>

      {/* WHO WE HELP */}
      <section className="home-section">
        <div className="home-section-header">
          <h2>Who We Help</h2>
          <p>Built for teams that need scale – from fast‑moving startups to global enterprises.</p>
        </div>

        <div className="home-grid two-col">
          <article className="home-card accent">
            <h3>Startups</h3>
            <p className="home-card-subtitle">
              Move from MVP to scale‑ready product with an architecture that will not slow you down later.
            </p>
            <ul>
              <li>MVPs in 30–60 days</li>
              <li>Cloud‑native architecture</li>
              <li>Growth‑ready systems</li>
              <li>Reduced order processing time by 40%</li>
              <li>Scaled to 10,000+ users &amp; 300,000+ products</li>
            </ul>
            <a href="/case-studies" className="btn-outline">
              View Case Study
            </a>
          </article>

          <article className="home-card">
            <h3>Enterprises</h3>
            <p className="home-card-subtitle">
              Modernize legacy systems and launch secure, compliant platforms your teams actually enjoy using.
            </p>
            <ul>
              <li>Legacy system modernization</li>
              <li>Secure microservices</li>
              <li>Compliance‑ready platforms</li>
              <li>Reduced manual ops by 60%</li>
              <li>Improved deployment speed by 3x</li>
              <li>Enhanced compliance tracking</li>
            </ul>
            <a href="/case-studies" className="btn-outline">
              View Case Study
            </a>
          </article>
        </div>
      </section>

      {/* CTA – BUILD TO SCALE */}
      <section className="home-section cta-band">
        <div className="home-cta-inner">
          <div>
            <h2>Let&apos;s Build Something That Scales</h2>
            <p>
              Schedule a call with our engineering leaders and get a roadmap for your next
              product or platform initiative.
            </p>
          </div>
          <a href="/contact" className="btn-primary">
            Get your free Architecture Review 
          </a>
        </div>
        <div className="home-logos">
          <span>RajeevsTech</span>
          <span>GreenPantry</span>
          <span>BangaruKottu</span>
        </div>
      </section>

      {/* WHY CHOOSE OMEGA TECHNOLOGIES */}
      <section className="home-section">
        <div className="home-section-header">
          <h2>Why Choose Omega Technologies</h2>
          <p>Quality, reliability, and partnerships you can count on.</p>
        </div>

        <div className="home-grid four-col">
          <div className="home-card">
            <h4>Enterprise First</h4>
            <p>Architecture built for security, scalability, and maintainability from day one.</p>
          </div>
          <div className="home-card">
            <h4>Proven Stack</h4>
            <p>.NET, React, SQL Server, Azure cloud – proven for complex domains.</p>
          </div>
          <div className="home-card">
            <h4>Ownership</h4>
            <p>Engineering teams that think like product owners, not vendors.</p>
          </div>
          <div className="home-card">
            <h4>Security &amp; Performance</h4>
            <p>Industry‑grade security and performance as a non‑negotiable baseline.</p>
          </div>
        </div>
      </section>

      {/* LEADERSHIP */}
      <section className="home-section home-founders">
        <div className="home-founders-grid">
          <article className="founder-card">
            <div className="founder-avatar raja" />
            <div className="founder-info">
              <h3>Rajeev Reddy</h3>
              <p className="founder-role">Founder</p>
              <p>
                Leads solution architecture and large‑scale delivery, bringing over a decade of
                experience in enterprise software.
              </p>
            </div>
          </article>

          <article className="founder-card">
            <div className="founder-avatar rahul" />
            <div className="founder-info">
              <h3>Indira </h3>
              <p className="founder-role">Director – Operations & Governance</p>
              <p>
              Oversees organizational operations, compliance, and internal governance to ensure 
              long-term stability and ethical business practices.
              </p>
            </div>
          </article>
        </div>
      </section>
    </>
  );
}

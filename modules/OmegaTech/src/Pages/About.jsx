import "../Components/styles.css";

export default function About(){
  return (
<section className="page page-alt">
  <div className="page-container">
    <h1>About Omega Technologies</h1>

    <p className="lead">
      We build scalable, secure, and business-critical digital platforms
      for startups and enterprises.
    </p>

    <details className="about-grid">
      <summary classname="about-card">
        <h4>What We Do</h4>
        <p>
          Full-stack development, cloud-native systems, and enterprise automation.
        </p>
      </summary>

      <summary classname="about-card">
        <h4>How We Work</h4>
        <p>
          Ownership-driven delivery with a strong focus on quality and performance.
        </p>
      </summary>
    </details>
  </div>
</section>
  );
}

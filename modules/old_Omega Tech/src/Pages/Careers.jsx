import { useState } from "react";

export default function Careers() {
  const [form, setForm] = useState({
    name: "",
    email: "",
    role: "",
    resumeLink: "",
    message: "",
  });

  const [showModal, setShowModal] = useState(false);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setShowModal(true);
  };

  const proceedToEmail = () => {
    const mailBody = `
Name: ${form.name}
Email: ${form.email}
Role: ${form.role}

Resume Link:
${form.resumeLink}

Message:
${form.message}
    `;

    window.location.href = `mailto:careers@omega-technologies.in?subject=${encodeURIComponent(
      `Job Application – ${form.role}`
    )}&body=${encodeURIComponent(mailBody)}`;
  };

  return (
    <div className="careers-root">
      <div>
        <h1>Careers at Omega Technologies</h1>
        <p>
          Join us in building scalable, secure, and modern digital solutions.
        </p>

        <section>
          <h2>Open Positions</h2>
          <ul>
            <li>Senior .NET Full Stack Developer</li>
            <li>React.js Frontend Engineer</li>
            <li>Azure Cloud Engineer</li>
          </ul>
        </section>

        <section>
          <h2>Apply Now</h2>

          <form onSubmit={handleSubmit}>
            <input name="name" placeholder="Full Name" required onChange={handleChange} />
            <input type="email" name="email" placeholder="Email Address" required onChange={handleChange} />
            <input name="role" placeholder="Role Applying For" required onChange={handleChange} />
            <input type="url" name="resumeLink" placeholder="Google Drive Resume Link" required onChange={handleChange} />

            <p>
              Upload your resume to Google Drive → Get shareable link → Set access
              to <b> Anyone with link (Viewer)</b>
            </p>

            <textarea name="message" rows="4" placeholder="Short introduction / portfolio link" onChange={handleChange} />

            <button type="submit">Submit Application</button>
          </form>
        </section>
      </div>

      {showModal && (
        <div className="fixed">
          <div>
            <h3>Almost Done! 🎉</h3>
            <p>Your email client will open with your application details.</p>
            <div className="flex">
              <button onClick={() => setShowModal(false)}>Cancel</button>
              <button onClick={proceedToEmail}>Proceed to Send Email</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

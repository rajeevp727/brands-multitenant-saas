import "./styles.css";

export default function Footer() {
  return (
    <footer className="footer">
      <div className="footer-bottom">
        © {new Date().getFullYear()}{" "}
        <span
          className="footer-brand-link"
          onClick={() => {
            if (window.location.pathname === "/") {
              window.location.reload();
            } else {
              window.location.href = "/";
            }
          }}
          role="link"
        >
          Omega Technologies Pvt. Ltd.
        </span>{" "}
        | All Rights Reserved
      </div>
    </footer>
  );
}

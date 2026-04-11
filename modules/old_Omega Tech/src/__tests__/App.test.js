import { render, screen } from "@testing-library/react";
import App from "../App";

test("renders Home page by default", () => {
  render(<App />);

  expect(
    screen.getByText(/Engineering Scalable Digital Solutions/i)
  ).toBeInTheDocument();
});

test("renders About page when navigating", () => {
  window.history.pushState({}, "About", "/about");

  render(<App />);

  expect(
    screen.getByText(/About Omega Technologies/i)
  ).toBeInTheDocument();
});

test("renders Contact page when navigating", () => {
  window.history.pushState({}, "Contact", "/contact");

  render(<App />);

  expect(screen.getByText(/Contact Us/i)).toBeInTheDocument();
});

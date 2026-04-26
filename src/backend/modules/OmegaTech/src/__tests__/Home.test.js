import { render, screen } from "@testing-library/react";
import Home from "../Pages/Home";

test("renders hero heading", () => {
  render(<Home />);
  expect(
    screen.getByText(/Engineering Scalable Digital Solutions/i)
  ).toBeInTheDocument();
});

test("renders CTA buttons", () => {
  render(<Home />);
  expect(screen.getByText(/Get a Quote/i)).toBeInTheDocument();
  expect(screen.getByText(/Learn More/i)).toBeInTheDocument();
});

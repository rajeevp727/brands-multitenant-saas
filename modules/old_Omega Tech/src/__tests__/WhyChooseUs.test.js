import { render, screen } from "@testing-library/react";
import WhyChooseUs from "../Components/WhyChooseUs";

test("renders Why Choose Us section", () => {
  render(<WhyChooseUs />);
  expect(
    screen.getByText(/Why Choose Omega Technologies/i)
  ).toBeInTheDocument();
});

test("renders trust cards", () => {
  render(<WhyChooseUs />);
  expect(screen.getByText(/Enterprise-First/i)).toBeInTheDocument();
  expect(screen.getByText(/Proven Stack/i)).toBeInTheDocument();
});

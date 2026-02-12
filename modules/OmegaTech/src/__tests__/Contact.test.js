import { render, screen } from "@testing-library/react";
import Contact from "../Pages/Contact";

test("renders contact page content", () => {
  render(<Contact />);
  expect(screen.getByText(/Contact Us/i)).toBeInTheDocument();
  expect(
    screen.getByText(/rajeevreddy@omegatechnologies.in/i)
  ).toBeInTheDocument();
});

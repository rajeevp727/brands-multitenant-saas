import { render, screen, fireEvent } from "@testing-library/react";
import Footer from "../Components/Footer";

test("renders footer brand name", () => {
  render(<Footer />);
  expect(
    screen.getByText(/Omega Technologies Pvt. Ltd./i)
  ).toBeInTheDocument();
});

test("reloads page when clicking brand on home", () => {
  delete window.location;
  window.location = { pathname: "/", reload: jest.fn() };

  render(<Footer />);
  fireEvent.click(screen.getByText(/Omega Technologies Pvt. Ltd./i));

  expect(window.location.reload).toHaveBeenCalled();
});

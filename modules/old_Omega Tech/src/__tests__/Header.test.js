import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import Header from "../Components/Header";

test("renders company logo and name", () => {
  render(
    <MemoryRouter>
      <Header />
    </MemoryRouter>
  );

  expect(screen.getByText("Omega Technologies")).toBeInTheDocument();
});

test("shows Contact as button when on /contact", () => {
  render(
    <MemoryRouter initialEntries={["/contact"]}>
      <Header />
    </MemoryRouter>
  );

  const contactButton = screen.getByText("Contact");
  expect(contactButton).toHaveClass("nav-cta");
});

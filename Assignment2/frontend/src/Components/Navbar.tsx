import { Link, useNavigate } from "react-router-dom";

export default function Navbar() {
  const navigate = useNavigate();

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <nav className="bg-blue-700 text-white p-4 flex justify-between items-center">
      <Link to="/dashboard" className="font-semibold text-lg">
        Project Manager
      </Link>
      <button
        onClick={logout}
        className="bg-red-500 hover:bg-red-600 px-3 py-1 rounded-lg"
      >
        Logout
      </button>
    </nav>
  );
}

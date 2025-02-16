import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { isAuthenticated, logout } from '../services/authService';

const Dashboard = () => {
  const [user, setUser] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await isAuthenticated();
      const role = localStorage.getItem('role');  // Pobieranie roli z localStorage
      if (!authenticated || !role) {
        navigate('/login');
      } else {
        if (role === 'Admin') {
          navigate('/admin-dashboard');  // Jeśli rola to admin, przekieruj na AdminDashboard
        } else {
          setUser('Zalogowany użytkownik');
        }
      }
    };

    checkAuth();
  }, [navigate]);

  const handleLogout = () => {
    logout(navigate);
  };

  return (
    <div className="dashboard-container">
      <h1>{user}</h1>
      <button onClick={handleLogout} className="logout-button">
        Wyloguj się
      </button>
    </div>
  );
};

export default Dashboard;

import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { isAuthenticated, logout } from '../services/authService';

const Dashboard = () => {
  const [user, setUser] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await isAuthenticated();
      if (!authenticated) {
        navigate('/login');
      } else {
        setUser('Zalogowany użytkownik');
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

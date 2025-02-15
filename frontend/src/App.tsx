import React, { useEffect } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
import Login from './pages/login';
import Dashboard from './pages/Dashboard';
import { isAuthenticated } from './services/authService';

const App = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await isAuthenticated();

      if (authenticated) {
        if (window.location.pathname === '/login') {
          navigate('/dashboard');
        }
      } else {
        if (window.location.pathname !== '/login') {
          navigate('/login');
        }
      }
    };

    checkAuth();
  }, [navigate]);

  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/dashboard" element={<Dashboard />} />
    </Routes>
  );
};

export default App;

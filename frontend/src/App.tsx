import React, { useEffect } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
import Login from './pages/login';
import Dashboard from './pages/Dashboard';
import Register from './pages/Register';
import { isAuthenticated } from './services/authService';
import AdminDashboard from './pages/AdminDashboard';

const App = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await isAuthenticated();
      const path = window.location.pathname;

      if (authenticated) {
        if (path === '/login' || path === '/register') {
          navigate('/dashboard'); // Jeśli zalogowany, przekieruj na dashboard
        }
      } else {
        if (path !== '/login' && path !== '/register') {
          navigate('/login'); // Jeśli niezalogowany, przekieruj na login
        }
      }
    };

    checkAuth();
  }, [navigate]);

  return (
    <Routes>
      <Route path="/register" element={<Register />} />
      <Route path="/login" element={<Login />} />
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/admin-dashboard" element={<AdminDashboard />} />
    </Routes>
  );
};

export default App;

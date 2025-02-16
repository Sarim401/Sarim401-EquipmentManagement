import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

const API_URL = 'http://localhost:5223/api/auth/';

export const login = async (email: string, password: string) => {
  try {
    const { data } = await axios.post(`${API_URL}login`, { email, password });
    localStorage.setItem('token', data.token);

    const decoded: any = jwtDecode(data.token);
    const userId = decoded?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    const role = decoded?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];  // Pobieranie roli użytkownika
    if (userId) {
      localStorage.setItem('userId', userId); 
    }
    if (role) {
      localStorage.setItem('role', role);
    }

    document.cookie = `refreshToken=${data.refreshToken}; Path=/;`;
    return data;
  } catch (error) {
    console.error('Błąd logowania', error);
    throw error;
  }
};


export const logout = (navigate: ReturnType<typeof useNavigate>) => {
  try {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('role');
    document.cookie = 'refreshToken=; Max-Age=0; Path=/;';
    navigate('/login');
  } catch (error) {
    console.error('Błąd wylogowania', error);
  }
};

export const refreshToken = async () => {
  const refreshToken = getCookie('refreshToken');
  if (!refreshToken) {
    throw new Error('Brak refresh tokenu');
  }

  try {
    const userId = localStorage.getItem('userId');
    if (!userId) {
      throw new Error('Nie znaleziono userId w localStorage');
    }

    const { data } = await axios.post(`${API_URL}refresh-token`, { refreshToken, userId });
    localStorage.setItem('token', data.token);
    return data.token;
  } catch (error) {
    console.error('Błąd odświeżania tokenu', error);
    throw error;
  }
};

const getCookie = (name: string) => {
  const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
  return match ? match[2] : null;
};

export const isAuthenticated = async () => {
  try {
    const token = localStorage.getItem('token');
    if (!token) {
      await refreshToken();
      return true;
    }

    const userId = localStorage.getItem('userId');
    if (userId) {
      return true;
    }

    return false;
  } catch (error) {
    console.error('Błąd weryfikacji autoryzacji', error);
    return false;  // Return false if any error occurs
  }
};

export const getToken = () => {
  return localStorage.getItem('token');
};

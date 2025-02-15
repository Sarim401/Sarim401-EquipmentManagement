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
      if (userId) {
        localStorage.setItem('userId', userId); 
      }

      document.cookie = `refreshToken=${data.refreshToken}; Path=/;`;
  
      return data;
    } catch (error) {
      console.error('Błąd logowania', error);
      throw error;
    }
  };
  
export const logout = (navigate: ReturnType<typeof useNavigate>) => {
  localStorage.removeItem('token');
  localStorage.removeItem('userId');
  document.cookie = 'refreshToken=; Max-Age=0; Path=/;';

  navigate('/login');
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

      console.log('Odpowiedź z serwera: ', data);

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
    const token = localStorage.getItem('token');
    if (!token) {
      try {
        await refreshToken();
        return true;
      } catch (error) {
        return false;
      }
    }
  
    const userId = localStorage.getItem('userId');
    if (userId) {
      return true;
    }
  
    return false;
  };

export const getToken = () => {
  return localStorage.getItem('token');
};

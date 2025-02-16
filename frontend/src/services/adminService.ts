import axios from 'axios';

const apiUrl = 'http://localhost:5223/api/users';  // Upewnij się, że masz poprawny URL

// Funkcja do pobierania listy użytkowników
export const getUsers = async () => {
  try {
    const token = localStorage.getItem('token'); // Pobranie tokenu JWT
    
    const response = await axios.get(`${apiUrl}`, {
      headers: {
        Authorization: `Bearer ${token}`  // Dodanie tokenu do nagłówka
      }
    });

    return response.data; // Zwrócenie listy użytkowników
  } catch (error) {
    throw new Error('Nie udało się pobrać użytkowników');
  }
};

// Funkcja do zmiany roli użytkownika
export const changeUserRole = async (data: { userId: string, newRole: string }) => {
  try {
    const token = localStorage.getItem('token'); // Pobranie tokenu JWT
    
    const response = await axios.put(`${apiUrl}/change-role`, data, {
      headers: {
        Authorization: `Bearer ${token}`  // Dodanie tokenu do nagłówka
      }
    });

    return response.data;
  } catch (error) {
    throw new Error('Nie udało się zmienić roli użytkownika');
  }
};

// Funkcja do usuwania użytkownika
export const deleteUser = async (userId: string) => {
  try {
    const token = localStorage.getItem('token'); // Pobranie tokenu JWT
    
    const response = await axios.delete(`${apiUrl}/${userId}`, {
      headers: {
        Authorization: `Bearer ${token}`  // Dodanie tokenu do nagłówka
      }
    });

    return response.data;
  } catch (error) {
    throw new Error('Nie udało się usunąć użytkownika');
  }
};

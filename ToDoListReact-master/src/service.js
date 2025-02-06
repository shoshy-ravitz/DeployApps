import axios from 'axios';

const apiUrl = "http://localhost:5159/";
axios.defaults.baseURL = apiUrl; // הגדרת כתובת ה-API כ-default

// הוספת interceptor לתפיסת שגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    console.error('API error:', error); // רישום השגיאה ללוג
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get("/tasks"); // עדכון ה-route
    return result.data;
  },

  addTask: async(name) => {
    console.log('addTask', name);
    const result = await axios.post("/tasks", { name }); // הוספת השם לגוף הבקשה
    return result.data;
  },

  setCompleted: async(id, isComplete) => {
    const result = await axios.put(`/tasks/${id}`, { IsComplet: isComplete }); // עדכון ה-route והגוף
    return result.data;
  },

  deleteTask: async(id) => {
    const result = await axios.delete(`/tasks/${id}`); // עדכון ה-route
    return result.data;
  }
};

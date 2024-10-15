import axios from 'axios';

const API_URL = 'https://d25c-1-6-46-135.ngrok-free.app/api/Users/login'; // Correct API URL

export const login = async (username, password) => {
  try {
    // Making the POST request using axios
    const response = await axios.post(API_URL, {
      username: username,
      password: password
    });

    // If the response is successful (status 200), return the token and roles
    if (response.status === 200) {
      const { token, roles } = response.data;

      //console.log('Login successful:', response.data);

      // Return the token and roles for further use
      return { token, roles };
    } else {
      // Handle cases where the response is not successful
      throw new Error('Login failed: Invalid credentials');
    }
  } catch (error) {
    console.error('Login API call error:', error.message);
    throw error; // Throw error to handle it in the LoginScreen
  }
};

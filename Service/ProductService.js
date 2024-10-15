import axios from 'axios';

const API_URL = 'https://f92b-1-6-46-135.ngrok-free.app/api/product'; // Replace with your actual API URL

export const fetchProducts = async () => {
  try {
    const response = await axios.get(API_URL);
    
    if (response.status === 200) {
      return response.data; // The array of products from the API
    } else {
      throw new Error('Failed to fetch products');
    }
  } catch (error) {
    console.error('Error fetching products:', error.message);
    throw error; // To handle the error in the HomeScreen
  }
};

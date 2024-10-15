import React, { useEffect, useState } from 'react';
import { View, Text, FlatList, Image, StyleSheet, ActivityIndicator, Button, TouchableOpacity } from 'react-native';
import axios from 'axios';
import Animated from 'react-native-reanimated'; // Import Reanimated for animations
import { FontAwesome } from '@expo/vector-icons'; // Cart Icon

const API_URL = 'https://d25c-1-6-46-135.ngrok-free.app/api/product';

const HomeScreen = ({ navigation, route }) => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [cart, setCart] = useState(route?.params?.cartItems || []); // Initialize cart with passed data

  // Fetch products from API
  useEffect(() => {
   
    fetchProducts();
  }, []);

// Update cart if new data is passed from CartScreen
useEffect(() => {
  if (route?.params?.cartItems) {
    setCart(route.params.cartItems);
  }
}, [route.params?.cartItems]);


  const fetchProducts = async () => {
    try {
      const response = await axios.get(API_URL);
      setProducts(response.data);
      setLoading(false);
    } catch (error) {
  debugger

      console.error('Error fetching products:', error);
      setLoading(false);
    }
  };


  // Update cart if new data is passed from CheckoutScreen
  useEffect(() => {
    if (route?.params?.cartItems) {
      setCart(route.params.cartItems);
    }
  }, [route.params?.cartItems]);

  // Add to cart functionality
  const addToCart = (product) => {
    setCart((prevCart) => [...prevCart, product]);
    alert(`${product.name} added to cart!`);
  };

  if (loading) {
    return <ActivityIndicator size="large" color="#0000ff" style={styles.loader} />;
  }

  const renderItem = ({ item }) => (
    <Animated.View style={styles.productCard}>
      <Image source={{ uri: `https://f92b-1-6-46-135.ngrok-free.app${item.images[0].imagePath}` }} style={styles.productImage} />
      <View style={styles.productDetails}>
        <Text style={styles.productName}>{item.name}</Text>
        <Text style={styles.productPrice}>${item.price}</Text>
        <Text style={styles.productDescription}>{item.description}</Text>
        {/* Add to Cart Button */}
        <TouchableOpacity onPress={() => addToCart(item)} style={styles.addToCartButton}>
          <Text style={styles.addToCartText}>Add to Cart</Text>
        </TouchableOpacity>
      </View>
    </Animated.View>
  );

  return (
    <View style={styles.container}>
      <TouchableOpacity style={styles.cartIcon} onPress={() => navigation.navigate('Cart', { cartItems: cart, setCart })}>
        <FontAwesome name="shopping-cart" size={28} color="#333" />
        {cart.length > 0 && (
          <View style={styles.cartBadge}>
            <Text style={styles.cartBadgeText}>{cart.length}</Text>
          </View>
        )}
      </TouchableOpacity>

      <Text style={styles.heading}>Our Products</Text>
      <FlatList
        data={products}
        renderItem={renderItem}
        keyExtractor={(item) => item.id.toString()}
        contentContainerStyle={styles.productList}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f8f8',
    paddingTop: 20,
    paddingHorizontal: 10,
  },
  heading: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    textAlign: 'center',
    marginBottom: 20,
  },
  productList: {
    paddingBottom: 50,
  },
  productCard: {
    backgroundColor: '#fff',
    borderRadius: 10,
    overflow: 'hidden',
    marginBottom: 20,
    elevation: 3, // Adds shadow for Android
    shadowColor: '#000', // Shadow for iOS
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
  },
  productImage: {
    width: '100%',
    height: 200,
    resizeMode: 'cover',
  },
  productDetails: {
    padding: 15,
  },
  productName: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 5,
  },
  productPrice: {
    fontSize: 18,
    color: '#e74c3c',
    marginBottom: 10,
  },
  productDescription: {
    fontSize: 14,
    color: '#777',
    marginBottom: 10,
  },
  addToCartButton: {
    backgroundColor: '#2ecc71',
    paddingVertical: 10,
    borderRadius: 5,
    marginTop: 10,
    alignItems: 'center',
  },
  addToCartText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  loader: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  cartIcon: {
    position: 'absolute',
    top: 40,
    right: 10,
    zIndex: 10,
    flexDirection: 'row',
    alignItems: 'center',
  },
  cartBadge: {
    backgroundColor: '#e74c3c',
    borderRadius: 10,
    width: 20,
    height: 20,
    justifyContent: 'center',
    alignItems: 'center',
    marginLeft: -10,
    marginTop: -5,
  },
  cartBadgeText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: 'bold',
  },
});

export default HomeScreen;

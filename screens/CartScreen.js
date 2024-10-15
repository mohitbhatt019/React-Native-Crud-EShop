import React, { useState } from 'react';
import { View, Text, FlatList, Button, StyleSheet, Image, TouchableOpacity } from 'react-native';
import { FontAwesome } from '@expo/vector-icons'; // Import icons
import { useNavigation } from '@react-navigation/native';

export default function CartScreen({ route }) {
  const { cartItems } = route.params;
  const [cart, setCart] = useState(cartItems.map(item => ({ ...item, quantity: 1 }))); // Add quantity to each item
  const navigation = useNavigation();

  const increaseQuantity = (id) => {
    setCart(prevCart =>
      prevCart.map(item =>
        item.id === id ? { ...item, quantity: item.quantity + 1 } : item
      )
    );
  };

  const decreaseQuantity = (id) => {
    setCart(prevCart =>
      prevCart.map(item =>
        item.id === id && item.quantity > 1 ? { ...item, quantity: item.quantity - 1 } : item
      )
    );
  };

  const removeItemFromCart = (id) => {
    const updatedCart = cart.filter(item => item.id !== id);
    setCart(updatedCart);
  
    // Pass the updated cart back to HomeScreen
    navigation.navigate('Home', { cartItems: updatedCart });
  };

  const handleCheckout = () => {
    navigation.push('Checkout', { cartItems: cart, setCart }); // Passing setCart function
  };

  const goToHome = () => {
    navigation.navigate('Home'); // Navigates to the Home screen
  };


  
  const renderItem = ({ item }) => (
    <View style={styles.cartItem}>
      <Image source={{ uri: `https://f92b-1-6-46-135.ngrok-free.app${item.images[0].imagePath}` }} style={styles.itemImage} />
      <View style={styles.itemDetails}>
        <Text style={styles.itemName}>{item.name}</Text>
        <Text style={styles.itemPrice}>${(item.price * item.quantity).toFixed(2)}</Text>

        <View style={styles.quantityContainer}>
          <TouchableOpacity onPress={() => decreaseQuantity(item.id)} style={styles.iconButton}>
            <FontAwesome name="minus" size={20} color="#333" />
          </TouchableOpacity>

          <Text style={styles.quantityText}>{item.quantity}</Text>

          <TouchableOpacity onPress={() => increaseQuantity(item.id)} style={styles.iconButton}>
            <FontAwesome name="plus" size={20} color="#333" />
          </TouchableOpacity>
        </View>
      </View>
      
      {/* Delete button */}
      <TouchableOpacity onPress={() => removeItemFromCart(item.id)} style={styles.deleteButton}>
        <FontAwesome name="trash" size={24} color="red" />
      </TouchableOpacity>
    </View>
  );

  return (
    <View style={styles.container}>
      <Text style={styles.heading}>Your Cart</Text>
      {cart.length > 0 ? (
        <FlatList
          data={cart}
          renderItem={renderItem}
          keyExtractor={(item) => item.id.toString()} // Ensure that keys are properly extracted as strings
        />
      ) : (
        <Text>Your cart is empty</Text>
      )}
      
      {cart.length > 0 ? (
        <Button title="Checkout" onPress={() => handleCheckout()} />
      ) : (
        <Button title="Go to Home" onPress={() => goToHome()} />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
  },
  heading: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
    textAlign: 'center',
  },
  cartItem: {
    marginVertical: 10,
    flexDirection: 'row',
    alignItems: 'center',
  },
  itemImage: {
    width: 80,
    height: 80,
    marginRight: 15,
    resizeMode: 'cover',
  },
  itemDetails: {
    flex: 1,
  },
  itemName: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  itemPrice: {
    fontSize: 16,
    color: '#e74c3c',
    marginVertical: 5,
  },
  quantityContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 10,
  },
  quantityText: {
    fontSize: 16,
    marginHorizontal: 10,
  },
  iconButton: {
    backgroundColor: '#ddd',
    padding: 5,
    borderRadius: 5,
  },
  deleteButton: {
    marginLeft: 10,
  },
});

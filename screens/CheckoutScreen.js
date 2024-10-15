    import React from 'react';
    import { View, Text, Button, StyleSheet, FlatList, Image } from 'react-native';

    export default function CheckoutScreen({ route, navigation }) {
        const { cartItems, setCart } = route.params;  // Pass setCart from HomeScreen
    
        // Calculate total price
        const totalPrice = cartItems.reduce((total, item) => total + item.price * item.quantity, 0);
    
        const handlePlaceOrder = () => {
            alert('Order placed successfully!');
            
            // Clear the cart
            setCart([]);  // Use setCart to empty the cart
        
            // Navigate back to Home, passing an empty cart
            navigation.navigate('Home', { cartItems: [] });
          };
    
        const renderItem = ({ item }) => (
        <View style={styles.orderItem}>
            <Image 
            source={{ uri: `https://f92b-1-6-46-135.ngrok-free.app${item.images[0].imagePath}` }} 
            style={styles.itemImage} 
            />
            <View style={styles.itemDetails}>
            <Text style={styles.itemName}>{item.name} (x{item.quantity})</Text>
            <Text style={styles.itemPrice}>${(item.price * item.quantity).toFixed(2)}</Text>
            </View>
        </View>
        );
    
        return (
        <View style={styles.container}>
            <Text style={styles.heading}>Checkout</Text>
            
            <Text style={styles.subheading}>Order Summary:</Text>
    
            <FlatList
            data={cartItems}
            renderItem={renderItem}
            keyExtractor={(item) => item.id.toString()}
            />
    
            <View style={styles.totalContainer}>
            <Text style={styles.totalText}>Total Price:</Text>
            <Text style={styles.totalPrice}>${totalPrice.toFixed(2)}</Text>
            </View>
    
            <Button title="Place Order" onPress={handlePlaceOrder} />
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
    subheading: {
        fontSize: 18,
        fontWeight: 'bold',
        marginBottom: 10,
    },
    orderItem: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginVertical: 10,
    },
    itemDetails: {
        flex: 1,
        marginLeft: 10,
    },
    itemName: {
        fontSize: 16,
    },
    itemPrice: {
        fontSize: 16,
        color: '#e74c3c',
    },
    itemImage: {
        width: 50,
        height: 50,
        borderRadius: 5,
    },
    totalContainer: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginVertical: 20,
    },
    totalText: {
        fontSize: 18,
        fontWeight: 'bold',
    },
    totalPrice: {
        fontSize: 18,
        fontWeight: 'bold',
        color: '#e74c3c',
    },
    });

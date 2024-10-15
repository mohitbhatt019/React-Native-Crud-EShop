import React from 'react';
import { View, Button } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { logout } from '../Service/AuthService'; // Assuming AuthService handles authentication

export default function ProfileScreen() {
  const navigation = useNavigation();

  const handleLogout = async () => {
    await logout();
    navigation.navigate('Login'); // Redirect to login screen after logout
  };

  return (
    <View>
      <Button title="Logout" onPress={handleLogout} />
    </View>
  );
}

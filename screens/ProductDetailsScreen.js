import React from 'react';
import { View, Text, Button, Image } from 'react-native';
import { useNavigation } from '@react-navigation/native';

export default function ProductDetailsScreen({ route }) {
  const { product } = route.params;
  const navigation = useNavigation();

  return (
    <View>
      <Image source={{ uri: product.images[0].imagePath }} style={{ width: 200, height: 200 }} />
      <Text>{product.name}</Text>
      <Text>{product.price}</Text>
      <Text>{product.description}</Text>
      <Button title="Add to Cart" onPress={() => navigation.goBack()} />
    </View>
  );
}

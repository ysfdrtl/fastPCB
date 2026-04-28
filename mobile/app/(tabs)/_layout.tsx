import { Ionicons } from "@expo/vector-icons";
import { createMaterialTopTabNavigator } from "@react-navigation/material-top-tabs";
import { withLayoutContext } from "expo-router";
import type { ComponentProps } from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { colors } from "@/theme/colors";

const { Navigator } = createMaterialTopTabNavigator();
const SwipeTabs = withLayoutContext(Navigator);

type IconName = ComponentProps<typeof Ionicons>["name"];

export default function TabsLayout() {
  const insets = useSafeAreaInsets();
  const bottomInset = Math.max(insets.bottom, 10);

  return (
    <SwipeTabs
      tabBarPosition="bottom"
      screenOptions={({ route }) => ({
        lazy: true,
        swipeEnabled: true,
        tabBarActiveTintColor: colors.primary,
        tabBarInactiveTintColor: colors.muted,
        tabBarShowIcon: true,
        tabBarPressColor: "#d9f8f3",
        tabBarIndicatorStyle: {
          backgroundColor: colors.primary,
          height: 3,
          borderRadius: 999
        },
        tabBarStyle: {
          backgroundColor: colors.surface,
          borderTopColor: colors.border,
          borderTopWidth: 1,
          minHeight: 62 + bottomInset,
          paddingBottom: bottomInset,
          elevation: 8,
          shadowColor: "#101828",
          shadowOpacity: 0.08,
          shadowOffset: { width: 0, height: -2 },
          shadowRadius: 10
        },
        tabBarLabelStyle: {
          fontSize: 11,
          fontWeight: "800",
          textTransform: "none"
        },
        tabBarItemStyle: {
          minHeight: 58,
          paddingTop: 7,
          paddingBottom: 5
        },
        tabBarIcon: ({ color, focused }) => (
          <Ionicons color={color} name={getTabIcon(route.name, focused)} size={22} />
        )
      })}
    >
      <SwipeTabs.Screen name="index" options={{ title: "Kesfet", tabBarLabel: "Kesfet" }} />
      <SwipeTabs.Screen name="upload" options={{ title: "Proje Yukle", tabBarLabel: "Yukle" }} />
      <SwipeTabs.Screen name="profile" options={{ title: "Profil", tabBarLabel: "Profil" }} />
    </SwipeTabs>
  );
}

function getTabIcon(routeName: string, focused: boolean): IconName {
  if (routeName === "upload") {
    return focused ? "cloud-upload" : "cloud-upload-outline";
  }

  if (routeName === "profile") {
    return focused ? "person-circle" : "person-circle-outline";
  }

  return focused ? "grid" : "grid-outline";
}

import { createSlice } from '@reduxjs/toolkit';

export const userSlice = createSlice({
  name: 'user',
  initialState: {
    UserId: 0,
    email: '',
    mobileNumber: '',
    role: '',
    firstName: '',
    lastName: '',
    profilePicUrl: '',
    birthDate: '',
  },
  reducers: {
    setUser: (state, action) => {
      return { ...state, ...action.payload };
    },
    desetUser: () => {
      return {
        UserId: 0,
        email: '',
        mobileNumber: '',
        role: '',
        firstName: '',
        lastName: '',
        profilePicUrl: '',
        birthDate: '',
      };
    },
  },
});

export const { setUser, desetUser } = userSlice.actions;

export default userSlice.reducer;

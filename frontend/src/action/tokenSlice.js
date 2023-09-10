import { createSlice } from '@reduxjs/toolkit';

export const tokenSlice = createSlice({
  name: 'tokens',
  initialState: {
    accessToken: '',
    refreshToken: '',
  },
  reducers: {
    setTokens: (state, action) => {
      return { ...state, ...action.payload };
    },
    clearTokens: (state) => {
      state.accessToken = '';
      state.refreshToken = '';
    },
  },
});

export const { setTokens, clearTokens } = tokenSlice.actions;

export default tokenSlice.reducer;

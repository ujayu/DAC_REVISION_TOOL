import { createSlice } from '@reduxjs/toolkit';

export const settingsSlice = createSlice({
  name: 'settings',
  initialState: {
    answerMode: "SideBySide",
  },
  reducers: {
    setAnswerMode: (state, action) => {
      state.answerMode = action.payload;
    },
  },
});

export const { setAnswerMode } = settingsSlice.actions;

export default settingsSlice.reducer;

import { configureStore } from '@reduxjs/toolkit'
import settingsReducer from '../action/settingsSlice'
import tokensReducer from '../action/tokenSlice'
import userReducer from '../action/userSlice'

export default configureStore({
  reducer: {
    user: userReducer,
    token: tokensReducer,
    settings: settingsReducer,
  },
})
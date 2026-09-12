import {createRouter, createWebHistory} from 'vue-router'
import BodyManagementView from '@/views/BodyManagementView.vue'

const router = createRouter({
    history:createWebHistory(),
    routes:[
        {
            path:'/body',
            name:'body',
            component:BodyManagementView
        }
    ]
})

export default router
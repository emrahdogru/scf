import intro from './views/intro/Main.vue'
import login from './views/intro/Login.vue'

export const routes = [
    // Giriş sayfaları
    {
        path: '/',
        component: intro,
        children:
        [
            {
                name: 'login',
                path: '',
                component: login
            },
            {
                name: 'forgetpassword',
                path: '/forgetpassword',
                component: () => import('./views/intro/ForgetPassword.vue')
            }
        ]

    },
    // NoTenant: Yetkili olduğunuz bir hesap yok
    {
        name: 'notenant',
        path: '/notenant',
        component: () => import('./views/NoTenant.vue')
    },
    {
        path: '/:tenantId',
        component: () => import('./views/Layout.vue'),
        children:
        [
            {
                name: 'dashboard',
                path: '',
                component: () => import('./views/Dashboard.vue')
            },

            // Yönetim
            {
                path: 'management',
                component: () => import('./views/management/Main.vue'),
                children:
                [
                    {
                        name: 'management',
                        path: '',
                        component: () => import('./views/management/Dashboard.vue')
                    },
                    {
                        name: 'employee',
                        path: 'employee',
                        component: () => import('./views/management/Employee.vue'),
                        children:
                        [
                            {
                                name: 'employeeForm',
                                path: ':id',
                                component: () => import('./views/management/EmployeeForm.vue')
                            }
                        ]
                    },
                    {
                        name: 'group',
                        path: 'group',
                        component: () => import('./views/management/Group.vue'),
                        children:
                        [
                            {
                                name: 'groupForm',
                                path: ':id',
                                component: () => import('./views/management/GroupForm.vue')
                            }
                        ]
                    },
                    {
                        name: 'position',
                        path: 'position',
                        component: () => import('./views/management/Position.vue'),
                        children:
                        [
                            {
                                name: 'positionForm',
                                path: ':id',
                                component: () => import('./views/management/PositionForm.vue')
                            }
                        ]
                    },
                    {
                        name: 'employeeTitle',
                        path: 'employeetitle',
                        component: () => import('./views/management/EmployeeTitle.vue'),
                        children:
                        [
                            {
                                name: 'employeeTitleForm',
                                path: ':id',
                                component: () => import('./views/management/EmployeeTitleForm.vue')
                            }
                        ]
                    },
                ]
            },

            // Prim
            {
                path: 'premium',
                component: () => import('./views/premium/Main.vue'),
                children:
                [
                    {
                        name: 'premium',
                        path: '',
                        component: () => import('./views/premium/Dashboard.vue')
                    },
                    {
                        name: 'premiumCycleDetail',
                        path: ':cycleId',
                        component: () => import('./views/premium/Detail.vue')
                    },
                    {
                        name: 'premiumCycleForm',
                        path: ':cycleId/form',
                        component: () => import('./views/premium/Form.vue')
                    },
                    {
                        name: 'premiumCycleFileList',
                        path: ':cycleId/files',
                        component: () => import('./views/premium/FileList.vue'),
                        children: 
                        [
                            {
                                name: 'premiumCycleFileForm',
                                path: ':id',
                                component: () => import('./views/premium/FileForm.vue')
                            },
                        ]
                    },

                ]
            }
        ]
    },
];

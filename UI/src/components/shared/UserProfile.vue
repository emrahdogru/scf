<script setup lang="ts">
import { IUserProfileSummary } from '../../models/abstractions';


var props = defineProps<{
    user: IUserProfileSummary
}>()

</script>
<template>
    <template v-if="props.user">
        {{ props.user.firstName }} {{ props.user.lastName }}
    </template>
</template>